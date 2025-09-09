package com.capstone.Arogya.model;

import jakarta.persistence.*;
import lombok.*;

import java.time.Instant;

@Entity
@Table(name = "answers",
       indexes = {@Index(columnList = "user_id"), @Index(columnList = "question_id")})
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@ToString
public class Answer {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "user_id", nullable = false)
    private Long userId;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "question_id", nullable = false)
    private Question question;

    // For STRING answers
    @Column(columnDefinition = "TEXT")
    private String answerText;

    // For INT answers
    private Integer answerInt;

    // For MCQ: link to selected option (nullable)
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "selected_option_id")
    private QuestionOption selectedOption;

    // when answered
    private Instant answeredAt;
}
