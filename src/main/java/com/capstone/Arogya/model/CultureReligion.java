package com.capstone.Arogya.model;

import jakarta.persistence.*;
import lombok.*;

import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "culture_religion")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
@ToString(onlyExplicitlyIncluded = true)
public class CultureReligion {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @EqualsAndHashCode.Include
    private Long id;

    @Column(nullable = false, unique = true)
    private String name;

    // optional bidirectional mapping
    @ManyToMany(mappedBy = "cultureAndReligion", fetch = FetchType.LAZY)
    private Set<User> users = new HashSet<>();

    private String religion;

    private String culturalBelief;

    private Boolean isReligious;
}
