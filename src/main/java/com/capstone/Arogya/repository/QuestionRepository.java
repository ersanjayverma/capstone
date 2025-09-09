package com.capstone.Arogya.repository;

import com.capstone.Arogya.model.Question;
import org.springframework.data.jpa.repository.JpaRepository;

public interface QuestionRepository extends JpaRepository<Question, Long> { }
